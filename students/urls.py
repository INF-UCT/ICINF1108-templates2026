from django.urls import path

from . import views

urlpatterns = [
    path("", views.students_list),
    path("<str:id>/", views.student_detail),
]
