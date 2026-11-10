import { ConflictException, Injectable, NotFoundException } from "@nestjs/common"
import { DataSource, EntityManager } from "typeorm"

import { Student } from "./students.entity"
import { StudentsRepository } from "./students.repository"
import { PetsRepository } from "@/pets/pets.repository"
import { CreateStudentDto, UpdateStudentDto } from "@/students/students.dtos"
import type { ValidationErrors } from "@/shared/response"

@Injectable()
export class StudentsService {
	constructor(
		private readonly dataSource: DataSource,
		private readonly studentsRepository: StudentsRepository,
		private readonly petsRepository: PetsRepository,
	) {}

	public findAll(): Promise<Student[]> {
		return this.studentsRepository.findAll()
	}

	public async findById(id: string): Promise<Student> {
		const student = await this.studentsRepository.findById(id)

		if (!student) {
			throw new NotFoundException("Estudiante no encontrado")
		}

		return student
	}

	public create(data: CreateStudentDto): Promise<Student> {
		// El estudiante y sus mascotas se insertan en una sola transacción: si
		// cualquier mascota falla, Postgres hace rollback y no queda nada guardado.
		return this.dataSource.transaction(async (manager) => {
			await this.assertNoConflicts(data, undefined, manager)

			const student = await this.studentsRepository.save(
				{
					name: data.name,
					username: data.username,
					email: data.email,
					age: data.age,
				},
				manager,
			)

			for (const petData of data.pets ?? []) {
				const existing = await this.petsRepository.findByStudentAndName(
					student.id,
					petData.name,
					manager,
				)

				if (existing) {
					throw new ConflictException({
						message: "Conflicto con datos existentes",
						errors: {
							name: { message: "Ya tienes una mascota con ese nombre" },
						},
					})
				}

				await this.petsRepository.save(
					{
						studentId: student.id,
						name: petData.name,
						species: petData.species,
						age: petData.age ?? null,
					},
					manager,
				)
			}

			return student
		})
	}

	public async update(id: string, data: UpdateStudentDto): Promise<Student> {
		const existing = await this.findById(id)
		await this.assertNoConflicts(data, id)

		return this.studentsRepository.save({
			...existing,
			name: data.name ?? existing.name,
			username: data.username ?? existing.username,
			email: data.email ?? existing.email,
			age: data.age ?? existing.age,
		})
	}

	public async delete(id: string): Promise<Student> {
		const existing = await this.findById(id)

		// Las mascotas se eliminan en cascada (ON DELETE CASCADE).
		await this.studentsRepository.delete(id)

		return existing
	}

	private async assertNoConflicts(
		data: { email?: string; username?: string },
		exceptId?: string,
		manager?: EntityManager,
	): Promise<void> {
		const conflicts: ValidationErrors = {}

		if (data.email) {
			const existing = await this.studentsRepository.findByEmail(data.email, manager)

			if (existing && existing.id !== exceptId) {
				conflicts.email = { message: "El correo electrónico ya está en uso" }
			}
		}

		if (data.username) {
			const existing = await this.studentsRepository.findByUsername(data.username, manager)

			if (existing && existing.id !== exceptId) {
				conflicts.username = { message: "El nombre de usuario ya está en uso" }
			}
		}

		if (Object.keys(conflicts).length > 0) {
			throw new ConflictException({
				message: "Conflicto con datos existentes",
				errors: conflicts,
			})
		}
	}
}
