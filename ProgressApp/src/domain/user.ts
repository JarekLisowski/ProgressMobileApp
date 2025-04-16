import { ICreateUserRequestDto, IUpdateUserRequestDto } from "./generated/apimodel";

export class User implements ICreateUserRequestDto, IUpdateUserRequestDto {
        id: string = "";
        userName: string = "";
        email: string = "";
        firstName: string = "";
        lastName: string = "";
        isActive: boolean = true;
        password: string = "";
}