import { useEffect, useState } from "react";
import axios from "axios";
import { Link } from "react-router-dom";
import APP_ENV from "../../env";
import Navbar from '../navbar'; // Import Navbar
import { FaEdit, FaTrash } from "react-icons/fa"; // Import edit and delete icons

const ProductPage = () => {
    const [products, setProducts] = useState([]); // Дерево продуктів

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const res = await axios.get(`${APP_ENV.URL}api/products`);
                console.log("Fetched products:", res.data); // Лог для перевірки продуктів
                setProducts(res.data);
            } catch (err) {
                console.error("Error fetching products:", err);
            }
        };

        fetchProducts(); // Завантаження продуктів
    }, []);

    const handleDelete = async (productId) => {
        console.log("Attempting to delete product with ID:", productId);
        if (window.confirm("Are you sure you want to delete this product?")) {
            try {
                const response = await axios.delete(`${APP_ENV.URL}api/products/${productId}`);
                console.log("Delete response:", response); // Log the response
                setProducts((prevProducts) => prevProducts.filter(product => product.id !== productId));
            } catch (err) {
                console.error("Error deleting product:", err); // Log the error details
            }
        }
    };

    return (
        <div className="container">
            <Navbar />
            <h1 className="text-center">Список продуктів</h1>
            <Link to={"/create-product"} className={"btn btn-primary mx-2"}>Додати продукт</Link>

            <div className="row mt-4">
                {products.map((product) => (
                    <div className="col-md-4 mb-4" key={product.id}>
                        <div className="card">
                            <img
                                src={`${APP_ENV.URL}images/150_${product.images[0]}`} // Product image
                                className="card-img-top"
                                alt={product.name}
                                style={{ height: "200px", objectFit: "cover" }} // Set height and crop image
                            />
                            <div className="card-body">
                                <h5 className="card-title">{product.name}</h5>
                                <p className="card-text"><strong>Ціна: {product.price} грн</strong></p>
                                <Link to={`/product/${product.id}`} className="btn btn-info">Деталі</Link>

                                {/* Edit and delete buttons with icons */}
                                <Link to={`/edit-product/${product.id}`} className="btn btn-warning mx-2">
                                    <FaEdit /> {/* Edit icon */}
                                </Link>
                                <button
                                    className="btn btn-danger"
                                    onClick={() => handleDelete(product.id)} // Delete product by ID
                                >
                                    <FaTrash /> {/* Delete icon */}
                                </button>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default ProductPage;
