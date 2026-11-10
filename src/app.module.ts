import { Module } from "@nestjs/common"
import { TypeOrmModule } from "@nestjs/typeorm"
import { AuthModule } from "@/auth/auth.module"
import { PetsModule } from "@/pets/pets.module"
import { StudentsModule } from "@/students/students.module"
import { config } from "@/shared/config"

@Module({
	imports: [
		TypeOrmModule.forRoot({
			type: "postgres",
			url: config.databaseUrl,
			autoLoadEntities: true,
			synchronize: config.databaseSynchronize,
		}),
		AuthModule,
		StudentsModule,
		PetsModule,
	],
})
export class AppModule {}
