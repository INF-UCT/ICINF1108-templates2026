import { Module } from "@nestjs/common"
import { TypeOrmModule } from "@nestjs/typeorm"
import { User } from "@/users/user.entity"
import { UsersRepository } from "@/users/users.repository"

@Module({
	imports: [TypeOrmModule.forFeature([User])],
	providers: [UsersRepository],
	exports: [UsersRepository],
})
export class UsersModule {}
