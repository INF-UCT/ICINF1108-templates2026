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

La API expone operaciones CRUD completas sobre estudiantes bajo `/api/students`:

| Metodo | Ruta                    | Descripcion              |
|--------|-------------------------|---------------------------|
| POST   | `/api/students`         | Crear un estudiante       |
| GET    | `/api/students`         | Listar todos los estudiantes |
| GET    | `/api/students/:id`     | Buscar un estudiante por id |
| PATCH  | `/api/students/:id`     | Actualizar un estudiante  |
| DELETE | `/api/students/:id`     | Eliminar un estudiante    |

## Modelo de datos

Cada estudiante tiene:

| Campo       | Tipo             | Descripcion                          |
|-------------|------------------|----------------------------------------|
| `id`        | UUID             | Generado automaticamente al crear     |
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

El unico error manejado en el template es `409 Conflict` (`{"error": "..."}`) cuando el `email` ya esta registrado en otro estudiante.


Las respuestas de exito devuelven **el JSON del recurso directamente**.

Los datos se guardan en `Data/students.json`.

## Documentacion interactiva

Swagger UI disponible en `http://localhost:5101/docs` 

También puedes probar los endpoints directamente desde el archivo `estudiantes_icinf.http`.
