import { Injectable } from "@nestjs/common"
import { InjectRepository } from "@nestjs/typeorm"
import { isUUID } from "class-validator"
import { EntityManager, Repository } from "typeorm"

import { Student } from "./students.entity"

@Injectable()
export class StudentsRepository {
	constructor(
		@InjectRepository(Student)
		private readonly repository: Repository<Student>,
	) {}

	public findAll(): Promise<Student[]> {
		return this.repository.find({ order: { createdAt: "DESC" } })
	}

	public async findById(id: string): Promise<Student | null> {
		// Un id que no es UUID haría fallar la consulta en Postgres; se trata
		// igual que un id inexistente para responder 404.
		if (!isUUID(id)) return null

		return this.repository.findOneBy({ id })
	}

	public findByEmail(email: string, manager?: EntityManager): Promise<Student | null> {
		return this.repo(manager).findOneBy({ email })
	}

	public findByUsername(username: string, manager?: EntityManager): Promise<Student | null> {
		return this.repo(manager).findOneBy({ username })
	}

	public save(student: Partial<Student>, manager?: EntityManager): Promise<Student> {
		return this.repo(manager).save(student)
	}

	public async delete(id: string): Promise<void> {
		await this.repository.delete({ id })
	}

	// Dentro de una transacción se usa el repositorio del EntityManager de esa
	// transacción; fuera de ella, el repositorio normal.
	private repo(manager?: EntityManager): Repository<Student> {
		return manager ? manager.getRepository(Student) : this.repository
	}
}
