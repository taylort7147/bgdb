import React, { useContext, useEffect, useState } from 'react';
import Switch from "react-switch";
import { AppContext } from '../../../AppContext';

export default LibrarySyncStateSwitch;
export function LibrarySyncStateSwitch({ libraryId }) {
    const { token } = useContext(AppContext);
    const [isChecked, setIsChecked] = useState(false);

    useEffect(() => {
        fetch(`/api/library/${libraryId}`, {
            method: "GET",
            headers: new Headers({
                'Authorization': `Bearer ${token.accessToken}`
            })
        })
            .then(response => response.json())
            .then(data => setIsChecked(data.isEnabled))
    }, [token, libraryId]);

    var handleChange = checked => {
        // Assume the operation will be successful
        setIsChecked(checked);

        // Now make the API call and update the state
        fetch(`/api/library/setsyncstate/${libraryId}`, {
            method: "POST",
            headers: new Headers({
                'Authorization': `Bearer ${token?.accessToken}`,
                'Content-Type': "application/json"
            }),
            body: checked
        })
            .then(response => response.json())
            .then(data => setIsChecked(data));
    }

    return (
        <Switch onChange={handleChange} checked={isChecked} />
    );
}
