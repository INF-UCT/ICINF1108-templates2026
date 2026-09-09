import { randomUUID } from "node:crypto"
import { ConflictException, Injectable, NotFoundException } from "@nestjs/common"
import { InjectRepository } from "@nestjs/typeorm"
import { Repository } from "typeorm"

import { Student } from "./students.entity"
import { CreateStudentDto, UpdateStudentDto } from "@/students/students.dtos"
import type { ValidationErrors } from "@/shared/response"

@Injectable()
export class StudentsService {
	constructor(
		@InjectRepository(Student)
		private readonly studentsRepository: Repository<Student>,
	) {}

	public findAll(): Promise<Student[]> {
		return this.studentsRepository.find({ order: { createdAt: "DESC" } })
	}

	public async findById(id: string): Promise<Student> {
		const student = await this.studentsRepository.findOne({ where: { id } })

		if (!student) {
			throw new NotFoundException("Estudiante no encontrado")
		}

		return student
	}

	public async create(data: CreateStudentDto): Promise<Student> {
		await this.assertNoConflicts(data)

		const student = this.studentsRepository.create({
			id: randomUUID(),
			name: data.name,
			username: data.username,
			email: data.email,
			age: data.age,
		})

		return this.studentsRepository.save(student)
	}

	public async update(id: string, data: UpdateStudentDto): Promise<Student> {
		const existing = await this.findById(id)
		await this.assertNoConflicts(data, id)

		Object.assign(existing, {
			name: data.name ?? existing.name,
			username: data.username ?? existing.username,
			email: data.email ?? existing.email,
			age: data.age ?? existing.age,
		})

		return this.studentsRepository.save(existing)
	}

	public async delete(id: string): Promise<Student> {
		const existing = await this.findById(id)
		await this.studentsRepository.delete(id)
		return existing
	}

	private async assertNoConflicts(
		data: { email?: string; username?: string },
		exceptId?: string,
	): Promise<void> {
		const conflicts: ValidationErrors = {}

		if (data.email) {
			const existing = await this.studentsRepository.findOne({ where: { email: data.email } })

			if (existing && existing.id !== exceptId) {
				conflicts.email = { message: "El correo electrónico ya está en uso" }
			}
		}

		if (data.username) {
			const existing = await this.studentsRepository.findOne({
				where: { username: data.username },
			})

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
