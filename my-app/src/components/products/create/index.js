import { Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import axios from "axios";
import APP_ENV from "../../../env";
import Navbar from "../../navbar"; // Імпортуємо Navbar

const ProductCreatePage = () => {
    const navigate = useNavigate();
    const [data, setData] = useState({
        name: "", // Назва продукту
        price: 0, // Ціна продукту
        categoryId: 1, // Ідентифікатор категорії
        images: [], // Список файлів зображень
        description: ""
    });

    // Обробник зміни текстових полів
    const handlerOnChange = (e) => {
        setData({ ...data, [e.target.name]: e.target.value });
    };

    // Обробник вибору файлів (для кількох зображень)
    const handlerOnFileChange = (e) => {
        setData({ ...data, images: e.target.files }); // Зберігаємо всі обрані файли
    };

    // Обробник надсилання форми
    const handlerOnSubmit = (e) => {
        e.preventDefault();

        // Створюємо об'єкт FormData
        const formData = new FormData();
        formData.append("name", data.name);
        formData.append("price", data.price);
        formData.append("description", data.description);
        formData.append("categoryId", data.categoryId);

        // Додаємо всі зображення до formData
        for (let i = 0; i < data.images.length; i++) {
            formData.append("Images", data.images[i]); // 'Images' — назва поля в ASP.NET
        }

        // Надсилання даних на сервер
        axios.post(`${APP_ENV.URL}api/products`, formData, {
            headers: {
                "Content-Type": "multipart/form-data"
            }
        })
            .then(() => {
                navigate("/products"); // Перенаправлення після успішного додавання на products
            })
            .catch(err => {
                console.error("Помилка при надсиланні даних:", err);
            });
    };

    // Обробник скасування
    const handlerOnCancel = () => {
        navigate("/products"); // Перенаправлення на products при скасуванні
    };

    return (
        <div className="container">
            <Navbar /> {/* Додаємо Navbar компонент */}
            <h1 className="text-center">Додати продукт</h1>
            <form onSubmit={handlerOnSubmit} className="col-md-6 offset-md-3">
                <div className="mb-3">
                    <label htmlFor="name" className="form-label">Назва</label>
                    <input
                        type="text"
                        className="form-control"
                        id="name"
                        name="name"
                        value={data.name}
                        onChange={handlerOnChange}
                    />
                </div>

                <div className="mb-3">
                    <label htmlFor="price" className="form-label">Ціна</label>
                    <input
                        type="text"
                        className="form-control"
                        id="price"
                        name="price"
                        value={data.price}
                        onChange={handlerOnChange}
                    />
                </div>

                <div className="mb-3">
                    <label htmlFor="images" className="form-label">Фото</label>
                    <input
                        className="form-control"
                        type="file"
                        id="images"
                        name="images"
                        multiple  // Дозволяє вибирати кілька файлів
                        onChange={handlerOnFileChange}
                    />
                </div>
                <div className="form-floating mb-3">
                    <textarea
                        className="form-control"
                        placeholder="Вкажіть опис"
                        name={"description"}
                        id="description"
                        value={data.description}
                        onChange={handlerOnChange}
                        style={{ height: "100px" }}
                    ></textarea>
                    <label htmlFor="description">Опис</label>
                </div>

                <div className="mb-3 d-flex justify-content-center">
                    <button type="button" onClick={handlerOnCancel} className="btn btn-info mx-2">Скасувати</button>
                    <button type="submit" className="btn btn-primary">Додати</button>
                </div>
            </form>
        </div>
    );
};

export default ProductCreatePage;
