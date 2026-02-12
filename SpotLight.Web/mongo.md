Projects:
{
  "_id": {
    "$oid": "698b58d6246b1d8bd8104da3"
  },
  "equipo_numero": 1,
  "title": "Eco-Dron",
  "description": "Dron que planta árboles...",
  "category": "Sustentabilidad",
  "videoUrl": "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
  "members": [
    "Juan Escobar",
    "Enrique Peña"
  ],
  "technologies": [
    "Web",
    "Angular",
    "React",
    "IoT"
  ],
  "stats": {
    "puntuacion_factibilidad": 85,
    "totalEvaluaciones": 1
  },
  "status": "activo",
  "createdAt": "2026-02-01T10:00:00Z"
}

Validacion:
{
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
        bsonType: 'string'
      },
      category: {
        bsonType: 'string'
      },
      videoUrl: {
        bsonType: 'string'
      },
      equipo_numero: {
        bsonType: 'number'
      },
      description: {
        bsonType: 'string'
      },
      metodologia: {
        bsonType: 'string'
      },
      tecnologias: {
        bsonType: 'array'
      },
      members: {
        bsonType: 'array'
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
        'enum': [
          'activo',
          'desactivado'
        ]
      },
      createdAt: {
        bsonType: 'string'
      }
    }
  }
}
