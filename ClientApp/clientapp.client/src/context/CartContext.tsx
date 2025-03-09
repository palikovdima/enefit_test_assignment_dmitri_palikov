import React, { createContext, useContext, useState, useEffect } from "react";
import { CART_CLEAR_URL, CART_FETCH_URL, CART_TOTAL_AMOUNT_URL } from '../config/config';

interface Product {
    id: number;
    name: string;
    price: number;
    quantity: number;
    imageSource: string;
}

interface CartContextType {
    cart: Product[];
    setCart: React.Dispatch<React.SetStateAction<Product[]>>;
    totalAmount: number;
    setTotalAmount: React.Dispatch<React.SetStateAction<number>>;
    clearCart: () => Promise<void>;
    fetchCart: () => Promise<void>;
    getTotalAmount: () => Promise<void>;
}

const CartContext = createContext<CartContextType | undefined>(undefined);

export const CartProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [cart, setCart] = useState<Product[]>([]);
    const [totalAmount, setTotalAmount] = useState<number>(0);

    useEffect(() => {
        fetchCart();
        getTotalAmount();
    }, []);

    const fetchCart = async () => {
        try {
            const response = await fetch(`${CART_FETCH_URL}`, {
                method: "GET",
                credentials: "include",
            });

            if (response.ok) {
                const data = await response.json();
                setCart(data);
            } else {
                console.error("Failed to fetch cart");
            }
        } catch (error) {
            console.error("Error fetching cart:", error);
        }
    };

    const clearCart = async () => {
        try {
            const response = await fetch(`${CART_CLEAR_URL}`, {
                method: "GET",
                credentials: "include",
            });

            if (response.ok) {
                setCart([]);
                getTotalAmount();
                console.log("Cart cleared successfully");
            } else {
                console.error("Failed to clear cart");
            }
        } catch (error) {
            console.error("Error clearing cart:", error);
        }
    };

    const getTotalAmount = async () => {
        try {
            const response = await fetch(`${CART_TOTAL_AMOUNT_URL}`, {
                method: "GET",
                credentials: "include",
            });

            if (response.ok) {
                const totalAmount = await response.json()
                console.log(`Total amount ${totalAmount}`);
                setTotalAmount(totalAmount);
            } else {
                console.error(`Failed to calculate total amount`);
            }
        } catch (error) {
            console.error(error);
        }
    }

    return (
        <CartContext.Provider value={{ cart, setCart, totalAmount, setTotalAmount, clearCart, fetchCart, getTotalAmount }}>
            {children}
        </CartContext.Provider>
    );
};

export const useCart = () => {
    const context = useContext(CartContext);
    if (!context) {
        throw new Error("useCart must be used within a CartProvider");
    }
    return context;
};
