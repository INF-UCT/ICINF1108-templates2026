import { forwardRef, Module } from "@nestjs/common"
import { TypeOrmModule } from "@nestjs/typeorm"
import { PetsService } from "@/pets/pets.service"
import { PetsController } from "@/pets/pets.controller"
import { Pet } from "@/pets/pets.entity"
import { StudentsModule } from "@/students/students.module"

@Module({
	imports: [TypeOrmModule.forFeature([Pet]), forwardRef(() => StudentsModule)],
	controllers: [PetsController],
	providers: [PetsService],
	exports: [PetsService],
})
export class PetsModule {}
