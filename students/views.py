import json

from django.http import HttpResponse, JsonResponse
from django.views.decorators.csrf import csrf_exempt
from django.views.decorators.http import require_http_methods

from . import repository


@require_http_methods(["GET", "POST"])
@csrf_exempt
def students_list(request):
    if request.method == "GET":
        return JsonResponse(repository.obtener_todos(), safe=False)

    datos = json.loads(request.body or "{}")

    if repository.obtener_por_email(datos["email"]) is not None:
        return JsonResponse({"error": "El email ya esta registrado."}, status=409)

    student = repository.crear(datos)
    return JsonResponse(student, status=201)


@require_http_methods(["GET", "PATCH", "DELETE"])
@csrf_exempt
def student_detail(request, id):
    if request.method == "GET":
        return JsonResponse(repository.obtener_por_id(id))

    if request.method == "PATCH":
        datos = json.loads(request.body or "{}")

        if "email" in datos and repository.obtener_por_email(datos["email"], excluir_id=id) is not None:
            return JsonResponse({"error": "El email ya esta registrado."}, status=409)

        student = repository.actualizar(id, datos)
        return JsonResponse(student)

    repository.eliminar(id)
    return HttpResponse(status=204)
