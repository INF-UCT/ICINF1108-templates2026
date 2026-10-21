import { randomUUID } from "node:crypto"
import { ConflictException, Injectable, NotFoundException } from "@nestjs/common"

import { Student } from "./students.entity"
import { StudentsRepository } from "./students.repository"
import { CreateStudentDto, UpdateStudentDto } from "@/students/students.dtos"
import type { ValidationErrors } from "@/shared/response"

@Injectable()
export class StudentsService {
	constructor(private readonly studentsRepository: StudentsRepository) {}

	public async findAll(): Promise<Student[]> {
		return this.studentsRepository
			.findAll()
			.sort((a, b) => b.createdAt.getTime() - a.createdAt.getTime())
	}

	public async findById(id: string): Promise<Student> {
		const student = this.studentsRepository.findById(id)

		if (!student) {
			throw new NotFoundException("Estudiante no encontrado")
		}

		return student
	}

	public async create(data: CreateStudentDto): Promise<Student> {
		this.assertNoConflicts(data)

		const now = new Date()
		const student: Student = {
			id: randomUUID(),
			name: data.name,
			username: data.username,
			email: data.email,
			age: data.age,
			createdAt: now,
			updatedAt: now,
		}

		return this.studentsRepository.save(student)
	}

	public async update(id: string, data: UpdateStudentDto): Promise<Student> {
		const existing = await this.findById(id)
		this.assertNoConflicts(data, id)

		const updated: Student = {
			...existing,
			name: data.name ?? existing.name,
			username: data.username ?? existing.username,
			email: data.email ?? existing.email,
			age: data.age ?? existing.age,
			updatedAt: new Date(),
		}

		return this.studentsRepository.save(updated)
	}

	public async delete(id: string): Promise<Student> {
		const existing = await this.findById(id)
		this.studentsRepository.delete(id)
		return existing
	}

	private assertNoConflicts(
		data: { email?: string; username?: string },
		exceptId?: string,
	): void {
		const conflicts: ValidationErrors = {}

		if (data.email) {
			const existing = this.studentsRepository.findByEmail(data.email)

			if (existing && existing.id !== exceptId) {
				conflicts.email = { message: "El correo electrónico ya está en uso" }
			}
		}

		if (data.username) {
			const existing = this.studentsRepository.findByUsername(data.username)

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
