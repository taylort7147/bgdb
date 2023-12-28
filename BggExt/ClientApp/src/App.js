import React from "react";
import { Route, Routes } from "react-router-dom";
import AppRoutes from "./AppRoutes";
import { Layout } from "./components/Layout";
import { useToken } from "./useToken";
import "./custom.css";
import { AppContext } from "./AppContext";

export default function App() {
    const { token, setToken, removeToken } = useToken();

    return (
        <>
            <AppContext.Provider value={
                {
                    token: token,
                    setToken: setToken,
                    removeToken: removeToken
                }}>
                <Layout>
                    <Routes>
                        {AppRoutes.map((route, index) => {
                            const { element, ...rest } = route;
                            return <Route key={index} {...rest} element={element} />;
                        })}
                    </Routes>
                </Layout>
            </AppContext.Provider>
        </>
    );
}
