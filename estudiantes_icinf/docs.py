from django.http import HttpResponse, JsonResponse

from .openapi import OPENAPI_SPEC

SWAGGER_HTML = """<!DOCTYPE html>
<html>
<head>
  <title>Students API - Docs</title>
  <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5/swagger-ui.css" />
</head>
<body>
  <div id="swagger-ui"></div>
  <script src="https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js"></script>
  <script>
    window.onload = () => {
      window.ui = SwaggerUIBundle({
        url: "/openapi.json",
        dom_id: "#swagger-ui",
      });
    };
  </script>
</body>
</html>
"""


def openapi_json(request):
    return JsonResponse(OPENAPI_SPEC)


def swagger_docs(request):
    return HttpResponse(SWAGGER_HTML)
