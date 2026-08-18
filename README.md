# ICINF1108-templates2026

API demostrativa con .NET (ASP.NET Core Minimal API) para practicar consultas con Postman u otro cliente HTTP.

## Requisitos

- .NET SDK 10

## Clonar el repositorio

```bash
git clone https://github.com/INF-UCT/ICINF1108-templates2026
cd ICINF1108-templates2026
git checkout estudiantes_icinf-dotnet
```

## Restaurar dependencias

```bash
dotnet restore
```

## Ejecutar el servidor

```bash
dotnet run
```

El servidor queda disponible en la URL que se muestre en consola (por ejemplo `http://localhost:5101`).

## Endpoints disponibles

| Metodo | Ruta                        | Descripcion                  |
|--------|-----------------------------|-------------------------------|
| GET    | `/api/estudiantes`          | Listar todos los estudiantes  |
| GET    | `/api/estudiantes/{id}`     | Obtener un estudiante por id  |
| POST   | `/api/estudiantes`          | Crear un estudiante           |
| PUT    | `/api/estudiantes/{id}`     | Actualizar un estudiante      |
| DELETE | `/api/estudiantes/{id}`     | Eliminar un estudiante        |

Ejemplo de body para `POST` y `PUT`:

```json
{
  "nombre": "Pedro",
  "apellido": "Diaz",
  "email": "pedro.diaz@alu.uct.cl",
  "carrera": "Ingenieria Civil Informatica"
}
```

Los datos se guardan en `Data/estudiantes.json`. Las requests con datos invalidos (campos vacios, email mal formado) devuelven `400 Bad Request` con el detalle de los errores.

