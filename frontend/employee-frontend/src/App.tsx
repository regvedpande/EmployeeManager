import React, { useContext } from "react";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AuthContext, AuthProvider } from "./context/AuthContext";
import Employees from "./pages/Employees";
import Login from "./pages/Login";
import Register from "./pages/Register";

// ProtectedRoute Wrapper
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
    const auth = useContext(AuthContext);
    
    if (!auth?.isAuthenticated) {
        return <Navigate to="/login" replace />;
    }
    
    return <>{children}</>;
};

function App() {
    return (
        <BrowserRouter>
            <AuthProvider>
                <Routes>
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    
                    <Route
                        path="/employees"
                        element={
                            <ProtectedRoute>
                                <Employees />
                            </ProtectedRoute>
                        }
                    />
                    
                    {/* Default redirect */}
                    <Route path="/" element={<Navigate to="/employees" replace />} />
                    
                    {/* Catch-all redirect */}
                    <Route path="*" element={<Navigate to="/login" replace />} />
                </Routes>
            </AuthProvider>
        </BrowserRouter>
    );
}

export default App;