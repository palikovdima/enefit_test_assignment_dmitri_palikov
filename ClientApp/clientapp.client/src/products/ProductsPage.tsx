import { useEffect, useState } from 'react';
import * as signalR from "@microsoft/signalr";
import './ProductsPage.css';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';
import { CART_HUB_URL, CART_FETCH_URL, PRODUCT_HUB_URL, PRODUCTS_FETCH_URL, CART_PAGE_URL } from '../config/config';

interface Product {
    id: number;
    name: string;
    price: number;
    quantity: number;
    imageSource: string;
}

function ProductsPage() {
    const [products, setProducts] = useState<Product[]>([]);
    const navigate = useNavigate();
    const { clearCart, getTotalAmount, totalAmount } = useCart();

    useEffect(() => {
        const fetchData = async () => {
            await populateProductsData();
        };

        fetchData();

        const refreshTotalAmount = async () => {
            await getTotalAmount();
        };

        refreshTotalAmount();

        const productConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${PRODUCT_HUB_URL}`)
            .withAutomaticReconnect()
            .build();

        productConnection.start()
            .then(() => console.log("Connected to SignalR"))
            .catch(err => console.error("SignalR Connection Error: ", err));

        productConnection.on("ProductUpdated", async () => {
            await populateProductsData();
        });

        const cartConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${CART_HUB_URL}`)
            .withAutomaticReconnect()
            .build();

        cartConnection.start()
            .then(() => console.log("Connected to SignalR"))
            .catch(err => console.error("SignalR Connection Error: ", err));

        cartConnection.on("CartUpdated", async () => {
            await populateProductsData();
        });

        return () => {
            productConnection.stop();
            cartConnection.stop();
        };
    }, []);

    return (
        <div className="app-container">
            <header>
                <h1>Bake Sale</h1>
            </header>
            <main className="content">
                {products.length === 0 ? (
                    <p><em>Loading... Please refresh once the ASP.NET backend has started.</em></p>
                ) : (
                    <div className="product-grid">
                        {products.map(product => (
                            <div className={`product-card ${product.quantity > 0 ? '' : 'disabled'}`}
                                key={product.id} onClick={() => product.quantity > 0 && addProductToCart(product.id)}>
                                {product.imageSource && (
                                    <img
                                        src={product.imageSource}
                                        alt={product.name}
                                        className="product-image"
                                        loading="lazy"
                                        onError={(e) => e.currentTarget.style.display = "none"}
                                    />
                                )}
                                <h2>{product.name}</h2>
                                <p>Price: {"\u20AC"}{product.price}</p>
                                <p>Quantity: {product.quantity}</p>
                            </div>
                        ))}
                    </div>
                )}
            </main>
            <footer className="footer">
                <div className="footer-content">
                    <p className="total-amount">Total Amount: {"\u20AC"}{totalAmount}</p>
                    <button className="checkout-button" onClick={() => navigate({ pathname:CART_PAGE_URL })}>View Cart</button>
                    <button className="reset-button" onClick={clearCart}>Clear Cart</button>
                </div>
            </footer>
        </div>
    );

    async function populateProductsData() {
        const response = await fetch(`${PRODUCTS_FETCH_URL}`, {
            credentials: 'include'
        });
        if (response.ok) {
            const data = await response.json();
            setProducts(data);
        } else {
            console.error("Failed to fetch products");
        }
    }

    async function addProductToCart(productId: number) {
        console.info(`Adding product ${productId} to cart...`);
        try {
            const response = await fetch(`${CART_FETCH_URL}/${productId}`, {
                method: "GET",
                credentials: "include",
            });

            if (response.ok) {
                console.log(`Product ${productId} added to cart successfully`);
                await populateProductsData();
                await getTotalAmount();
            } else {
                console.error(`Failed to add product ${productId} to cart`);
            }
        } catch (error) {
            console.error(error);
        }
    }

}

export default ProductsPage;
