import { Column, CreateDateColumn, Entity, PrimaryColumn, Unique, UpdateDateColumn } from "typeorm"

@Entity("pets")
@Unique(["studentId", "name"])
export class Pet {
	@PrimaryColumn()
	id!: string

	@Column()
	studentId!: string

	@Column()
	name!: string

	@Column()
	species!: string

	@Column({ type: "int", nullable: true })
	age?: number

	@CreateDateColumn()
	createdAt!: Date

	@UpdateDateColumn()
	updatedAt!: Date
}
