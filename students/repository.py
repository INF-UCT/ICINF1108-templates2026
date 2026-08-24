import json
import os
import uuid
from datetime import datetime, timezone

DATA_FILE = os.path.join(os.path.dirname(__file__), "data", "students.json")


def _now():
    return datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")


def _leer_todos():
    if not os.path.exists(DATA_FILE):
        return []
    with open(DATA_FILE, "r", encoding="utf-8") as f:
        return json.load(f)


def _escribir_todos(students):
    with open(DATA_FILE, "w", encoding="utf-8") as f:
        json.dump(students, f, indent=2, ensure_ascii=False)


def obtener_todos():
    return _leer_todos()


def obtener_por_id(id):
    return next((s for s in _leer_todos() if s["id"] == id), None)


def obtener_por_email(email, excluir_id=None):
    return next(
        (s for s in _leer_todos() if s["email"].lower() == email.lower() and s["id"] != excluir_id),
        None,
    )


def crear(datos):
    students = _leer_todos()
    ahora = _now()
    student = {
        "id": str(uuid.uuid4()),
        "name": datos["name"],
        "email": datos["email"],
        "age": datos["age"],
        "createdAt": ahora,
        "updatedAt": ahora,
    }
    students.append(student)
    _escribir_todos(students)
    return student


def actualizar(id, datos):
    students = _leer_todos()
    student = next((s for s in students if s["id"] == id), None)
    if student is None:
        return None

    if "name" in datos:
        student["name"] = datos["name"]
    if "email" in datos:
        student["email"] = datos["email"]
    if "age" in datos:
        student["age"] = datos["age"]
    student["updatedAt"] = _now()

    _escribir_todos(students)
    return student


def eliminar(id):
    students = _leer_todos()
    restantes = [s for s in students if s["id"] != id]
    if len(restantes) == len(students):
        return False

    _escribir_todos(restantes)
    return True
