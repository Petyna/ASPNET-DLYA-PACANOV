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
    });

    // Обробник зміни текстових полів
    const handlerOnChange = (e) => {
        setData({ ...data, [e.target.name]: e.target.value });
    };

    // Обробник вибору файлів (для одного зображення)
    const handlerOnFileChange = (e) => {
        const file = e.target.files[0]; // Отримуємо перший обраний файл
        if (file) {
            setData((prevState) => ({
                ...prevState,
                images: [...prevState.images, file], // Додаємо файл до масиву зображень
            }));
        }
        e.target.value = null; // Очищення поля вводу
    };

    // Видалення непотрібного файлу
    const handleRemoveImage = (index) => {
        setData((prevState) => {
            const newImages = [...prevState.images];
            newImages.splice(index, 1); // Видаляємо зображення за індексом
            return { ...prevState, images: newImages };
        });
    };

    // Обробник надсилання форми
    const handlerOnSubmit = (e) => {
        e.preventDefault();

        // Створюємо об'єкт FormData
        const formData = new FormData();
        formData.append("name", data.name);
        formData.append("price", data.price);
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
                const addMore = window.confirm("Продукт додано!");
                if (addMore) {
                    setData({ ...data, images: [] }); // Очищаємо масив зображень для нового завантаження
                } else {
                    navigate("/products"); // Перенаправлення на список продуктів
                }
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
                        type="number" // Зміна типу на number для правильної обробки цін
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
                        onChange={handlerOnFileChange}
                    />
                </div>

                <div className="mb-3 d-flex justify-content-center">
                    <button type="button" onClick={handlerOnCancel} className="btn btn-info mx-2">Скасувати</button>
                    <button type="submit" className="btn btn-primary">Додати</button>
                </div>
            </form>

            {/* Відображення завантажених зображень */}
            {data.images.length > 0 && (
                <div>
                    <h3>Завантажені зображення:</h3>
                    <ul className="list-unstyled">
                        {data.images.map((image, index) => (
                            <li key={index} className="mb-2">
                                <div className="d-flex align-items-center">
                                    <img
                                        src={URL.createObjectURL(image)} // Використовуємо URL.createObjectURL для попереднього перегляду
                                        alt={image.name}
                                        style={{ width: "100px", height: "100px", objectFit: "cover", marginRight: "10px" }}
                                    />
                                    <span>{image.name}</span>
                                    <button className="btn btn-danger btn-sm ms-2" onClick={() => handleRemoveImage(index)}>
                                        Видалити
                                    </button>
                                </div>
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
};

export default ProductCreatePage;
