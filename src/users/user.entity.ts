import {
	Column,
	CreateDateColumn,
	Entity,
	PrimaryGeneratedColumn,
	UpdateDateColumn,
} from "typeorm"

// Usuarios que pueden iniciar sesión en la API (no confundir con Student,
// que es el recurso del CRUD).
@Entity("users")
export class User {
	@PrimaryGeneratedColumn("uuid")
	id!: string

	@Column({ type: "varchar", length: 100 })
	name!: string

	@Column({ type: "varchar", length: 255, unique: true })
	email!: string

	// `select: false`: el hash nunca se carga salvo que se pida explícitamente.
	@Column({ name: "password_hash", type: "varchar", length: 255, select: false })
	passwordHash!: string

	@CreateDateColumn({ name: "created_at", type: "timestamptz" })
	createdAt!: Date

	@UpdateDateColumn({ name: "updated_at", type: "timestamptz" })
	updatedAt!: Date
}

export type PublicUser = Omit<User, "passwordHash">
