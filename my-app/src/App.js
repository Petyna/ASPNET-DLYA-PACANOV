import './App.css';
import HomePage from "./components/home";
import { Route, Routes } from "react-router-dom";
import CategoryCreatePage from "./components/category/create";
import ProductCreatePage from "./components/products/create";
import ProductPage from "./components/products/index.js";
import ProductEditPage from "./components/products/edit";
import ProductDetailsPage from "./components/products/details";

const apiUrl = process.env.REACT_APP_API_URL;

function App() {
    console.log('API URL:', apiUrl);

    return (
        <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="create-category" element={<CategoryCreatePage />} />
            <Route path="create-product" element={<ProductCreatePage />} />
            <Route path="products" element={<ProductPage />} />
            <Route path="/edit-product/:id" element={<ProductEditPage />} />
            <Route path="/product/:id" element={<ProductDetailsPage />} />
        </Routes>
    );
}

export default App;
