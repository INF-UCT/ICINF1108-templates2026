# ICINF1108-templates2026

API demostrativa con Django para practicar consultas con Postman u otro cliente HTTP.


## Requisitos

- Python 3.9 o superior

## Clonar el repositorio y cambiar de rama

```bash
git clone https://github.com/INF-UCT/ICINF1108-templates2026
cd ICINF1108-templates2026
git checkout estudiantes_icinf-django
```

## Crear y activar el entorno virtual

macOS / Linux:

```bash
python3 -m venv .venv
source .venv/bin/activate
```

Windows (PowerShell):

```powershell
python -m venv .venv
.venv\Scripts\Activate.ps1
```

## Instalar dependencias

```bash
pip install -r requirements.txt
```

## Ejecutar el servidor

```bash
python manage.py runserver
```

El servidor queda disponible en `http://localhost:8000`.

## Endpoints disponibles

La API expone operaciones CRUD completas sobre estudiantes bajo `/api/students`:

| Metodo | Ruta                    | Descripcion              |
|--------|-------------------------|---------------------------|
| POST   | `/api/students`         | Crear un estudiante       |
| GET    | `/api/students`         | Listar todos los estudiantes |
| GET    | `/api/students/:id`     | Buscar un estudiante por id |
| PATCH  | `/api/students/:id`     | Actualizar un estudiante  |
| DELETE | `/api/students/:id`     | Eliminar un estudiante    |

En Django la ruta lleva slash final (`/api/students/`).

## Modelo de datos

Cada estudiante tiene:

| Campo       | Tipo             | Descripcion                          |
|-------------|------------------|----------------------------------------|
| `id`        | UUID             | Generado automaticamente al crear      |
| `name`      | string           | 3 a 100 caracteres                     |
| `email`     | string           | Direccion de correo valida y **unica** |
| `age`       | int              | Entre 18 y 99                          |
| `createdAt` | datetime (ISO)   | Generado automaticamente al crear      |
| `updatedAt` | datetime (ISO)   | Actualizado en cada `PATCH`            |

Ejemplo de body para `POST`:

```json
{
  "name": "Pedro Diaz",
  "email": "pedro.diaz@alu.uct.cl",
  "age": 22
}
```

`PATCH` acepta cualquier subconjunto de `name`, `email`, `age` (actualizacion parcial).

## Validaciones y errores

La validacion de entrada es manual (`students/validation.py`), equivalente en reglas a `class-validator`:

- `name`: texto de 3 a 100 caracteres, sin etiquetas HTML.
- `email`: direccion de correo valida.
- `age`: entero entre 18 y 99.


Las respuestas de exito devuelven **el JSON del recurso directamente**.

Los datos se guardan en `students/data/students.json`.

## Documentacion interactiva

Swagger UI disponible en `http://localhost:8000/docs/` 
