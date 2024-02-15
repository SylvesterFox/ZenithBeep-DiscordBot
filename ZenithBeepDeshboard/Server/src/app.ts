import * as express from "express"
import router from "./routers/router"
import { AppDataSource } from "./data-source"

AppDataSource
    .initialize()
    .then(() => {
        console.log("Data Source has been initialized!")
    })
    .catch ((err) =>
        {
            console.error("Error during Data Source initialization:", err)
        }
    )

const app = express()
app.use(express.json())
app.use('/api', router)

const run = async () => {
    try {
        app.listen(3000, () => console.log(`Server started`));
    } catch (e) {
        console.log(e);
    }
}

run();