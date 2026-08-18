import json

from django.http import HttpResponse, JsonResponse
from django.views.decorators.csrf import csrf_exempt
from django.views.decorators.http import require_http_methods

from . import repository


@require_http_methods(["GET", "POST"])
@csrf_exempt
def estudiantes_list(request):
    if request.method == "GET":
        return JsonResponse(repository.obtener_todos(), safe=False)

    datos = json.loads(request.body or "{}")
    estudiante = repository.crear(datos)
    return JsonResponse(estudiante, status=201)


@require_http_methods(["GET", "PUT", "DELETE"])
@csrf_exempt
def estudiante_detail(request, id):
    if request.method == "GET":
        estudiante = repository.obtener_por_id(id)
        if estudiante is None:
            return JsonResponse({"error": "Estudiante no encontrado"}, status=404)
        return JsonResponse(estudiante)

    if request.method == "PUT":
        datos = json.loads(request.body or "{}")
        estudiante = repository.actualizar(id, datos)
        if estudiante is None:
            return JsonResponse({"error": "Estudiante no encontrado"}, status=404)
        return JsonResponse(estudiante)

    eliminado = repository.eliminar(id)
    if not eliminado:
        return JsonResponse({"error": "Estudiante no encontrado"}, status=404)
    return HttpResponse(status=204)
