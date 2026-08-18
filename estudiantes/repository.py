import json
import os

DATA_FILE = os.path.join(os.path.dirname(__file__), "data", "estudiantes.json")


def _leer_todos():
    if not os.path.exists(DATA_FILE):
        return []
    with open(DATA_FILE, "r", encoding="utf-8") as f:
        return json.load(f)


def _escribir_todos(estudiantes):
    with open(DATA_FILE, "w", encoding="utf-8") as f:
        json.dump(estudiantes, f, indent=2, ensure_ascii=False)


def obtener_todos():
    return _leer_todos()


def obtener_por_id(id):
    return next((e for e in _leer_todos() if e["id"] == id), None)


def crear(datos):
    estudiantes = _leer_todos()
    nuevo_id = max((e["id"] for e in estudiantes), default=0) + 1
    estudiante = {
        "id": nuevo_id,
        "nombre": datos.get("nombre", ""),
        "apellido": datos.get("apellido", ""),
        "email": datos.get("email", ""),
        "carrera": datos.get("carrera", ""),
    }
    estudiantes.append(estudiante)
    _escribir_todos(estudiantes)
    return estudiante


def actualizar(id, datos):
    estudiantes = _leer_todos()
    estudiante = next((e for e in estudiantes if e["id"] == id), None)
    if estudiante is None:
        return None

    estudiante["nombre"] = datos.get("nombre", estudiante["nombre"])
    estudiante["apellido"] = datos.get("apellido", estudiante["apellido"])
    estudiante["email"] = datos.get("email", estudiante["email"])
    estudiante["carrera"] = datos.get("carrera", estudiante["carrera"])

    _escribir_todos(estudiantes)
    return estudiante


def eliminar(id):
    estudiantes = _leer_todos()
    restantes = [e for e in estudiantes if e["id"] != id]
    if len(restantes) == len(estudiantes):
        return False

    _escribir_todos(restantes)
    return True
