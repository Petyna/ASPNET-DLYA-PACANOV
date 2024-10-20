import { useEffect, useState } from "react";
import axios from "axios";
import { useParams, useNavigate } from "react-router-dom";
import APP_ENV from "../../../env";
import Navbar from '../../navbar';

const EditProductPage = () => {
    const { id } = useParams(); // Отримання ID продукту з URL
    const navigate = useNavigate(); // Для перенаправлення після успішного редагування

    const [product, setProduct] = useState({
        name: "",
        description: "",
        price: "",
        categoryId: "", // Вибрана категорія продукту
        images: [], // Зберігає файли для оновлення зображень
    });
    const [categories, setCategories] = useState([]); // Доступні категорії
    const [loading, setLoading] = useState(true);

    // Завантаження даних продукту за ID
    useEffect(() => {
        const fetchProduct = async () => {
            try {
                const res = await axios.get(`${APP_ENV.URL}api/products/${id}`);
                setProduct(res.data);
                setLoading(false);
            } catch (err) {
                console.error("Помилка завантаження продукту:", err);
                setLoading(false);
            }
        };

        // Завантаження категорій
        const fetchCategories = async () => {
            try {
                const res = await axios.get(`${APP_ENV.URL}api/categories`);
                setCategories(res.data);
            } catch (err) {
                console.error("Помилка завантаження категорій:", err);
            }
        };

        fetchProduct();
        fetchCategories();
    }, [id]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setProduct((prevProduct) => ({
            ...prevProduct,
            [name]: value,
        }));
    };

    const handleFileChange = (e) => {
        setProduct((prevProduct) => ({
            ...prevProduct,
            images: e.target.files, // Зберігаємо вибрані файли
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const formData = new FormData();

        // Додаємо поля тільки якщо вони не порожні
        if (product.name) formData.append("Name", product.name);
        if (product.price) formData.append("Price", product.price);
        if (product.categoryId) formData.append("CategoryId", product.categoryId);

        // Додаємо зображення якщо вибрані нові
        if (product.images && product.images.length > 0) {
            for (let i = 0; i < product.images.length; i++) {
                formData.append("Images", product.images[i]);
            }
        }

        try {
            await axios.put(`${APP_ENV.URL}api/products/${id}`, formData, {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            });
            alert("Продукт успішно відредаговано");
            navigate("/products");
        } catch (err) {
            console.error("Помилка при редагуванні продукту:", err.response ? err.response.data : err.message);
            alert("Помилка при редагуванні продукту: " + (err.response ? err.response.data : err.message));
        }
    };

    if (loading) {
        return <div>Завантаження...</div>;
    }

    return (
        <div className="container">
            <Navbar />
            <h1 className="text-center mt-3">Редагувати продукт</h1>
            <form onSubmit={handleSubmit}>
                <div className="form-group mt-3">
                    <label htmlFor="name">Назва продукту</label>
                    <input
                        type="text"
                        className="form-control"
                        id="name"
                        name="name"
                        value={product.name}
                        onChange={handleInputChange}
                        required
                    />
                </div>

                <div className="form-group mt-3">
                    <label htmlFor="price">Ціна продукту</label>
                    <input
                        type="number"
                        className="form-control"
                        id="price"
                        name="price"
                        value={product.price}
                        onChange={handleInputChange}
                        required
                    />
                </div>

                <div className="form-group mt-3">
                    <label htmlFor="category">Категорія продукту</label>
                    <select
                        className="form-control"
                        id="category"
                        name="categoryId"
                        value={product.categoryId}
                        onChange={handleInputChange}
                        required
                    >
                        <option value="">Виберіть категорію</option>
                        {categories.map((category) => (
                            <option key={category.id} value={category.id}>
                                {category.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="form-group mt-3">
                    <label htmlFor="images">Оновити зображення</label>
                    <input
                        type="file"
                        className="form-control"
                        id="images"
                        name="images"
                        multiple
                        onChange={handleFileChange}
                    />
                </div>

                <button type="submit" className="btn btn-success mt-3">
                    Зберегти зміни
                </button>
            </form>
        </div>
    );
};

export default EditProductPage;
