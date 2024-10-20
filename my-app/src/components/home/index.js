import { useEffect, useState } from "react";
import axios from "axios";
import { Link } from "react-router-dom";
import APP_ENV from "../../env";
import Navbar from '../navbar'; // Импортируем Navbar

const HomePage = () => {
    const [list, setList] = useState([]);

    useEffect(() => {
        axios.get(`${APP_ENV.URL}api/categories`)
            .then(res => {
                setList(res.data);
            });
    }, []);

    console.log("List items", list);

    return (
        <>
            <Navbar /> {/* Добавляем Navbar с правильным регистром */}
            <div className="container">
                <h1 className="text-center">Список категорій</h1>
                {/* Кнопка для добавления категории */}
                <Link to={"/create-category"} className={"btn btn-success mx-2"}>Додати категорію</Link>

                {/* Кнопка для добавления продукта */}
                <Link to={"/create-product"} className={"btn btn-primary mx-2"}>Додати продукт</Link>

                <table className="table mt-4">
                    <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">Фото</th>
                        <th scope="col">Назва</th>
                        <th scope="col">Опис</th>
                    </tr>
                    </thead>
                    <tbody>
                    {list.map((item) => (
                        <tr key={item.id}>
                            <th scope="row">{item.id}</th>
                            <td>
                                <img src={`${APP_ENV.URL}images/150_${item.image}`} alt={item.name} width="75px" />
                            </td>
                            <td>{item.name}</td>
                            <td>{item.description}</td>
                        </tr>
                    ))}
                    </tbody>
                </table>
            </div>
        </>
    )
}

export default HomePage;
