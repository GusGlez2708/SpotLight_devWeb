**# Entidades finales

Database Name: SpotLightDB

## Colletion: projects

{

  "equipo_numero": 1,

  "title": "Eco-Dron",

  "category": "Sustentabilidad",

  "description": "Dron que planta árboles...",

  "metodologia": "Agile",

  "tecnologias": ["Flutter", ".NET 8", "MongoDB"],

  "videoUrl": "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",

  "stats": { "averageScore": 0.0, "totalVotes": 0 },

  "createdAt": "2026-02-01T10:00:00Z"

}

Validacion:

{

  "$jsonSchema": {

    "bsonType": "object",

    "required": ["title", "category", "videoUrl", "equipo_numero"],

    "properties": {

    "title": { "bsonType": "string" },

    "category": { "bsonType": "string" },

    "videoUrl": { "bsonType": "string" },

    "equipo_numero": { "bsonType": "number" },

    "puntuacion_actual": { "bsonType": "number" }

    }

  }

}

## Colletion: evaluations

{

  "projectId": "ID_DEL_PROYECTO",

  "evaluatorId": "ID_DEL_MAESTRO",

  "scores": { "innovation": 5, "funcionalidad": 4, "diseno_ux": 5, "impacto": 5 },

  "finalScore": 19.0,

  "resena_texto": "Excelente propuesta técnica.",

  "aiAnalysis": {

    "puntuacion_factibilidad": 85,

    "fortalezas": ["Innovación", "Uso de IA"],

    "nivel_riesgo": "Bajo"

  }

}

Validacion:

{

  "$jsonSchema": {

    "bsonType": "object",

    "required": ["projectId", "evaluatorId", "scores"],

    "properties": {

    "projectId": { "bsonType": "string" },

    "evaluatorId": { "bsonType": "string" },

    "scores": {

    "bsonType": "object",

    "required": ["innovacion", "funcionalidad", "diseno_ux", "impacto"],

    "properties": {

    "innovacion": { "bsonType": "number", "minimum": 0, "maximum": 5 },

    "funcionalidad": { "bsonType": "number", "minimum": 0, "maximum": 5 },

    "diseno_ux": { "bsonType": "number", "minimum": 0, "maximum": 5 },

    "impacto": { "bsonType": "number", "minimum": 0, "maximum": 5 }

    }

    },

    "finalScore": { "bsonType": "number" }

    }

  }

}

## Colletion: users

{

  "nombre_completo": "Admin SpotLight",

  "correo_institucional": "admin@uady.mx",

  "password": "HashedPassword123",

  "rol": "evaluador",

  "area_especialidad": "Arquitectura",

  "status_verificacion": true

}

Validacion:

{

  "$jsonSchema": {

    "bsonType": "object",

    "required": ["nombre_completo", "correo_institucional", "rol", "status_verificacion"],

    "properties": {

    "nombre_completo": { "bsonType": "string" },

    "correo_institucional": { "bsonType": "string" },

    "rol": { "enum": ["evaluador", "estudiante"] },

    "status_verificacion": { "bsonType": "bool" },

    "stats": {

    "bsonType": "object",

    "properties": {

    "evaluaciones_completadas": { "bsonType": "number" }

    }

    }

    }

  }

}

## Colletion: ranking_global

{

  "ultima_actualizacion": "2026-02-04T16:00:00Z",

  "top_proyectos": [

    { "id_proyecto": "ID_1", "nombre": "Eco-Dron", "score_ia": 95, "posicion": 1 }

  ]

}

Validacion:

{

  "$jsonSchema": {

    "bsonType": "object",

    "required": ["ultima_actualizacion", "top_proyectos"],

    "properties": {

    "ultima_actualizacion": { "bsonType": "string" },

    "top_proyectos": { "bsonType": "array" }

    }

  }

}

**
