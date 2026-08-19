import {
  Body,
  Controller,
  Delete,
  Get,
  Param,
  Patch,
  Post,
} from "@nestjs/common";

import { PetsService } from "@/pets/pets.service";
import { CreatePetDto, UpdatePetDto } from "@/pets/pets.dtos";

@Controller("api/students/:studentId/pets")
export class PetsController {
  constructor(private readonly petsService: PetsService) {}

  @Get()
  public findAll(@Param("studentId") studentId: string) {
    const pets = this.petsService.findAllForStudent(studentId);
    return {
      total: pets.length,
      items: pets,
    };
  }

  @Post()
  public create(
    @Param("studentId") studentId: string,
    @Body() body: CreatePetDto,
  ) {
    return {
      ok: true,
      payload: this.petsService.create(studentId, body),
    };
  }

  @Patch(":petId")
  public update(
    @Param("studentId") studentId: string,
    @Param("petId") petId: string,
    @Body() body: UpdatePetDto,
  ) {
    return {
      ok: true,
      payload: this.petsService.update(studentId, petId, body),
    };
  }

  @Delete(":petId")
  public delete(
    @Param("studentId") studentId: string,
    @Param("petId") petId: string,
  ) {
    return {
      ok: true,
      payload: this.petsService.delete(studentId, petId),
    };
  }
}
