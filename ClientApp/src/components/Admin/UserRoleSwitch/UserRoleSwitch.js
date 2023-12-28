import React from 'react';
import Switch from "react-switch";
import { Label } from 'reactstrap';

export default UserRoleSwitch;
export function UserRoleSwitch({ role, checked, onChange }) {
    return (
        <div className="row">
            <Switch className="col-auto" onChange={onChange} checked={checked} />
            <Label className="col">{role}</Label>
        </div>
    );
}
