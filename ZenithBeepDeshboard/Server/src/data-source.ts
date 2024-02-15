import { DataSource } from "typeorm"

export const AppDataSource = new DataSource({
    type: "postgres",
    host: "localhost",
    port: 5432,
    username:"postgres",
    password: "0270",
    database: "grechkadb",
    entities: ["src/entity/*.js"],
    logging: true,
    synchronize: false
})