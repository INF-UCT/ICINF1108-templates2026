from django.contrib import admin
from django.urls import include, path

from . import docs

urlpatterns = [
    path('admin/', admin.site.urls),
    path('api/students/', include('students.urls')),
    path('openapi.json', docs.openapi_json),
    path('docs/', docs.swagger_docs),
]
