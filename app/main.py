from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from app.pets.pets_router import create_pets_router
from app.pets.pets_service import PetsService
from app.students.students_router import create_students_router
from app.students.students_service import StudentsService


def create_app() -> FastAPI:
    students_service = StudentsService()
    pets_service = PetsService(students_service)

    app = FastAPI(
        title="FastAPI CRUD Students & Pets",
        description=(
            "API de un CRUD en memoria para la entidad Student y sus mascotas (Pet)"
        ),
        version="1.0",
    )

    app.add_middleware(
        CORSMiddleware,
        allow_origins=["*"],
        allow_methods=["*"],
        allow_headers=["*"],
    )

    app.include_router(create_students_router(students_service, pets_service))
    app.include_router(create_pets_router(pets_service))

    return app


app = create_app()
