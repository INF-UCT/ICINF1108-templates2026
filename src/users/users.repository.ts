import { Injectable } from "@nestjs/common"
import { InjectRepository } from "@nestjs/typeorm"
import { isUUID } from "class-validator"
import { Repository } from "typeorm"

import { User } from "./user.entity"

@Injectable()
export class UsersRepository {
	constructor(
		@InjectRepository(User)
		private readonly repository: Repository<User>,
	) {}

	public async findById(id: string): Promise<User | null> {
		if (!isUUID(id)) return null

		return this.repository.findOneBy({ id })
	}

	public findByEmail(email: string): Promise<User | null> {
		return this.repository.findOneBy({ email })
	}

	// Única consulta que incluye el hash de la contraseña (para el login).
	public findByEmailWithPassword(email: string): Promise<User | null> {
		return this.repository
			.createQueryBuilder("user")
			.addSelect("user.passwordHash")
			.where("user.email = :email", { email })
			.getOne()
	}

	public save(user: Partial<User>): Promise<User> {
		return this.repository.save(user)
	}
}
