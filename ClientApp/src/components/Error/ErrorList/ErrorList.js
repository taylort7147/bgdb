import React from "react"
import "./ErrorList.css"

export default ErrorList;
export function ErrorList({ errors }) {
    return <ul className="error-list">
        {errors.map((error, i) => <li key={i} className="error-message">{error}</li>)}
    </ul>
}
