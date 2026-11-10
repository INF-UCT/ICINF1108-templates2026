import { Injectable } from "@nestjs/common"
import { InjectRepository } from "@nestjs/typeorm"
import { isUUID } from "class-validator"
import { EntityManager, Repository } from "typeorm"

import { Pet } from "./pets.entity"

@Injectable()
export class PetsRepository {
	constructor(
		@InjectRepository(Pet)
		private readonly repository: Repository<Pet>,
	) {}

	public findAllByStudent(studentId: string): Promise<Pet[]> {
		return this.repository.find({ where: { studentId }, order: { createdAt: "DESC" } })
	}

	public async findById(id: string): Promise<Pet | null> {
		// Un id que no es UUID haría fallar la consulta en Postgres; se trata
		// igual que un id inexistente para responder 404.
		if (!isUUID(id)) return null

		return this.repository.findOneBy({ id })
	}

	public findByStudentAndName(
		studentId: string,
		name: string,
		manager?: EntityManager,
	): Promise<Pet | null> {
		return this.repo(manager).findOneBy({ studentId, name })
	}

	public save(pet: Partial<Pet>, manager?: EntityManager): Promise<Pet> {
		return this.repo(manager).save(pet)
	}

	public async delete(id: string): Promise<void> {
		await this.repository.delete({ id })
	}

	private repo(manager?: EntityManager): Repository<Pet> {
		return manager ? manager.getRepository(Pet) : this.repository
	}
}
