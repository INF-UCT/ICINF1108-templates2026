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

| Metodo | Ruta                        | Descripcion               |
|--------|-----------------------------|----------------------------|
| GET    | `/api/estudiantes/`         | Listar todos los estudiantes |
| GET    | `/api/estudiantes/<id>/`    | Obtener un estudiante por id |
| POST   | `/api/estudiantes/`         | Crear un estudiante        |
| PUT    | `/api/estudiantes/<id>/`    | Actualizar un estudiante   |
| DELETE | `/api/estudiantes/<id>/`    | Eliminar un estudiante     |

Ejemplo de body para `POST` y `PUT`:

```json
{
  "nombre": "Pedro",
  "apellido": "Diaz",
  "email": "pedro.diaz@alu.uct.cl",
  "carrera": "Ingenieria Civil Informatica"
}
```

Los datos se guardan en `estudiantes/data/estudiantes.json`.
