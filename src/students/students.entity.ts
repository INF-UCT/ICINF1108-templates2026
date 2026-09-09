import { Column, CreateDateColumn, Entity, PrimaryColumn, UpdateDateColumn } from "typeorm"

@Entity("students")
export class Student {
	@PrimaryColumn()
	id!: string

	@Column()
	name!: string

	@Column({ unique: true })
	username!: string

	@Column({ unique: true })
	email!: string

	@Column({ type: "int" })
	age!: number

	@CreateDateColumn()
	createdAt!: Date

	@UpdateDateColumn()
	updatedAt!: Date
}
