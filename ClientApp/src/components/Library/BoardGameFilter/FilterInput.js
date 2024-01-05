export default FilterInput;
export function FilterInput({ param, type, handleChange }) {
    return (
        <input className="form-control"
            type={type}
            value={param ?? ""}
            onChange={handleChange}></input>
    );
}
