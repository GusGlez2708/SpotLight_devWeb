// 1. Actualizar documentos existentes para tener el campo imageUrl (opcional, setea un placeholder o null)
db.Projects.updateMany(
    { imageUrl: { $exists: false } },
    { $set: { imageUrl: "" } } // O una URL por defecto si prefieres
);

// 2. Actualizar el Validador del Schema (runCommand)
db.runCommand({
    collMod: "Projects",
    validator: {
        $jsonSchema: {
            bsonType: 'object',
            required: [
                'title',
                'category',
                'videoUrl',
                'equipo_numero'
            ],
            properties: {
                title: {
                    bsonType: 'string',
                    description: "Debe ser un string y es obligatorio"
                },
                category: {
                    bsonType: 'string',
                    description: "Debe ser un string y es obligatorio"
                },
                videoUrl: {
                    bsonType: 'string',
                    description: "Debe ser un string y es obligatorio"
                },
                imageUrl: {
                    bsonType: 'string',
                    description: "URL de la imagen para la App Móvil"
                },
                equipo_numero: {
                    bsonType: 'number',
                    description: "Debe ser un número y es obligatorio"
                },
                description: {
                    bsonType: 'string'
                },
                technologies: {
                    bsonType: 'array',
                    items: {
                        bsonType: "string"
                    }
                },
                members: {
                    bsonType: 'array',
                    items: {
                        bsonType: "string"
                    }
                },
                stats: {
                    bsonType: 'object',
                    properties: {
                        totalEvaluaciones: {
                            bsonType: 'number'
                        },
                        puntuacion_factibilidad: {
                            bsonType: 'number'
                        }
                    }
                },
                status: {
                    bsonType: 'string',
                    enum: [
                        'activo',
                        'desactivado'
                    ]
                },
                createdAt: {
                    bsonType: 'string'
                }
            }
        }
    },
    validationLevel: "strict",
    validationAction: "error"
});
