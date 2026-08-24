STUDENT_SCHEMA = {
    "type": "object",
    "properties": {
        "id": {"type": "string", "format": "uuid"},
        "name": {"type": "string", "minLength": 3, "maxLength": 100},
        "email": {"type": "string", "format": "email"},
        "age": {"type": "integer", "minimum": 18, "maximum": 99},
        "createdAt": {"type": "string", "format": "date-time"},
        "updatedAt": {"type": "string", "format": "date-time"},
    },
}

STUDENT_INPUT_SCHEMA = {
    "type": "object",
    "required": ["name", "email", "age"],
    "properties": {
        "name": {"type": "string", "minLength": 3, "maxLength": 100},
        "email": {"type": "string", "format": "email"},
        "age": {"type": "integer", "minimum": 18, "maximum": 99},
    },
}

OPENAPI_SPEC = {
    "openapi": "3.0.3",
    "info": {"title": "Students API", "version": "1.0.0"},
    "paths": {
        "/api/students": {
            "get": {
                "summary": "Listar estudiantes",
                "responses": {"200": {"description": "OK"}},
            },
            "post": {
                "summary": "Crear estudiante",
                "requestBody": {
                    "required": True,
                    "content": {"application/json": {"schema": STUDENT_INPUT_SCHEMA}},
                },
                "responses": {
                    "201": {"description": "Creado"},
                    "409": {"description": "Email ya registrado"},
                },
            },
        },
        "/api/students/{id}": {
            "get": {
                "summary": "Obtener estudiante por id",
                "parameters": [{"name": "id", "in": "path", "required": True, "schema": {"type": "string"}}],
                "responses": {"200": {"description": "OK"}},
            },
            "patch": {
                "summary": "Actualizar estudiante",
                "parameters": [{"name": "id", "in": "path", "required": True, "schema": {"type": "string"}}],
                "requestBody": {
                    "required": True,
                    "content": {"application/json": {"schema": STUDENT_SCHEMA}},
                },
                "responses": {
                    "200": {"description": "OK"},
                    "409": {"description": "Email ya registrado"},
                },
            },
            "delete": {
                "summary": "Eliminar estudiante",
                "parameters": [{"name": "id", "in": "path", "required": True, "schema": {"type": "string"}}],
                "responses": {"204": {"description": "Eliminado"}},
            },
        },
    },
    "components": {"schemas": {"Student": STUDENT_SCHEMA}},
}
