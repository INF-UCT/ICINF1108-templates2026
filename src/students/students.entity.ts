import {
	Column,
	CreateDateColumn,
	Entity,
	PrimaryGeneratedColumn,
	UpdateDateColumn,
} from "typeorm"

@Entity("students")
export class Student {
	@PrimaryGeneratedColumn("uuid")
	id!: string

	@Column({ type: "varchar", length: 100 })
	name!: string

	@Column({ type: "varchar", length: 30, unique: true })
	username!: string

	@Column({ type: "varchar", length: 255, unique: true })
	email!: string

	@Column({ type: "int" })
	age!: number

	@CreateDateColumn({ name: "created_at", type: "timestamptz" })
	createdAt!: Date

	@UpdateDateColumn({ name: "updated_at", type: "timestamptz" })
	updatedAt!: Date
}
