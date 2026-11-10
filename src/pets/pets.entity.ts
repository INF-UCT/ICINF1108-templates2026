import {
	Column,
	CreateDateColumn,
	Entity,
	JoinColumn,
	ManyToOne,
	PrimaryGeneratedColumn,
	Unique,
	UpdateDateColumn,
} from "typeorm"

import { Student } from "@/students/students.entity"

@Entity("pets")
@Unique("uq_pets_student_name", ["studentId", "name"])
export class Pet {
	@PrimaryGeneratedColumn("uuid")
	id!: string

	@Column({ name: "student_id", type: "uuid" })
	studentId!: string

	// Al eliminar un estudiante, Postgres elimina sus mascotas (ON DELETE CASCADE).
	@ManyToOne(() => Student, { onDelete: "CASCADE" })
	@JoinColumn({ name: "student_id" })
	student?: Student

	@Column({ type: "varchar", length: 50 })
	name!: string

	@Column({ type: "varchar", length: 50 })
	species!: string

	@Column({ type: "int", nullable: true })
	age!: number | null

	@CreateDateColumn({ name: "created_at", type: "timestamptz" })
	createdAt!: Date

	@UpdateDateColumn({ name: "updated_at", type: "timestamptz" })
	updatedAt!: Date
}
