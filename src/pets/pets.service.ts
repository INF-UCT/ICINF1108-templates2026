import { randomUUID } from "node:crypto"
import { InjectRepository } from "@nestjs/typeorm"
import { ConflictException, Injectable, NotFoundException } from "@nestjs/common"

import { Pet } from "./pets.entity"
import { Repository } from "typeorm"
import { CreatePetDto, UpdatePetDto } from "@/pets/pets.dtos"
import { StudentsService } from "@/students/students.service"

@Injectable()
export class PetsService {
	constructor(
		@InjectRepository(Pet)
		private readonly petsRepository: Repository<Pet>,
		private readonly studentsService: StudentsService,
	) {}

	public async findAllForStudent(studentId: string): Promise<Pet[]> {
		await this.assertStudentExists(studentId)

		return this.petsRepository.find({
			where: { studentId },
			order: { createdAt: "DESC" },
		})
	}

	public async create(studentId: string, data: CreatePetDto): Promise<Pet> {
		await this.assertStudentExists(studentId)
		await this.assertNoConflicts(studentId, data)

		const pet = this.petsRepository.create({
			id: randomUUID(),
			studentId,
			name: data.name,
			species: data.species,
			age: data.age,
		})

		return this.petsRepository.save(pet)
	}

	public async update(studentId: string, petId: string, data: UpdatePetDto): Promise<Pet> {
		const existing = await this.findOwned(studentId, petId)
		await this.assertNoConflicts(studentId, data, petId)

		Object.assign(existing, {
			name: data.name ?? existing.name,
			species: data.species ?? existing.species,
			age: data.age ?? existing.age,
		})

		return this.petsRepository.save(existing)
	}

	public async delete(studentId: string, petId: string): Promise<Pet> {
		const existing = await this.findOwned(studentId, petId)
		await this.petsRepository.delete(petId)
		return existing
	}

	public async deleteAllForStudent(studentId: string): Promise<void> {
		await this.petsRepository.delete({ studentId })
	}

	private async findOwned(studentId: string, petId: string): Promise<Pet> {
		await this.assertStudentExists(studentId)

		const pet = await this.petsRepository.findOne({ where: { id: petId } })

		if (!pet || pet.studentId !== studentId) {
			throw new NotFoundException("Mascota no encontrada")
		}

		return pet
	}

	private async assertStudentExists(studentId: string): Promise<void> {
		await this.studentsService.findById(studentId)
	}

	private async assertNoConflicts(
		studentId: string,
		data: { name?: string },
		exceptId?: string,
	): Promise<void> {
		if (!data.name) return

		const existing = await this.petsRepository.findOne({
			where: { studentId, name: data.name },
		})

		if (existing && existing.id !== exceptId) {
			throw new ConflictException({
				message: "Conflicto con datos existentes",
				errors: {
					name: { message: "Ya tienes una mascota con ese nombre" },
				},
			})
		}
	}
}
