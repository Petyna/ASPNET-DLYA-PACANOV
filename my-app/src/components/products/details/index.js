import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import axios from "axios";
import APP_ENV from "../../../env"; // Переконайтеся, що цей файл існує
import Navbar from '../../navbar';

const ProductDetailsPage = () => {
    const { id } = useParams(); // Отримуємо ідентифікатор продукту з URL
    const [product, setProduct] = useState(null);

    useEffect(() => {
        const fetchProduct = async () => {
            try {
                const res = await axios.get(`${APP_ENV.URL}api/products/${id}`);
                setProduct(res.data); // Зберігаємо дані продукту
            } catch (err) {
                console.error("Error fetching product details:", err);
            }
        };

        fetchProduct();
    }, [id]);

    if (!product) {
        return <div>Can't load :(</div>; // Коли продукт не завантажується
    }

    return (
        <div className="container">
            <Navbar />
            <h1 className="text-center">{product.name}</h1>
            <div className="card">
                <img
                    src={`${APP_ENV.URL}images/300_${product.productImages[0]}`} // Зображення продукту
                    className="card-img-top"
                    alt={product.name}
                    style={{ height: "400px", objectFit: "cover" }} // Встановлюємо розмір і обрізку зображення
                />
                <div className="card-body">
                    <h5 className="card-title">{product.name}</h5>
                    <p className="card-text">{product.description}</p> {/* Оновлено для відображення опису */}
                    <p className="card-text"><strong>Ціна: {product.price} грн</strong></p>
                    <Link to="/" className="btn btn-secondary">Назад до списку</Link>
                </div>
            </div>
        </div>
    );
};

export default ProductDetailsPage;
