import { IconX } from "../../Icons/Icons";

export default AppliedFilterButton  ;
export function AppliedFilterButton({ criterion, onClose }) {
    return (
        <button
            className="d-flex btn btn-primary bgdb-applied-filter me-1 mb-1"
            onClick={onClose}
        >{criterion.describe()}<IconX className="ms-2"/></button>
    );
}
