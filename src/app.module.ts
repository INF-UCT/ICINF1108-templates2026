import { Module } from "@nestjs/common"
import { TypeOrmModule } from "@nestjs/typeorm"
import { PetsModule } from "@/pets/pets.module"
import { StudentsModule } from "@/students/students.module"

@Module({
	imports: [
		TypeOrmModule.forRoot({
			type: "better-sqlite3",
			database: "data.db",
			autoLoadEntities: true,
			synchronize: true,
		}),
		StudentsModule,
		PetsModule,
	],
})
export class AppModule {}
