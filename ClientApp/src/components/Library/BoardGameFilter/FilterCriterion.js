export class FilterCriterion {
    constructor({ name, value, test, description }) {
        this.name = name;
        this._value = value;
        this._test = test;
        this._description = description;
    }

    clone() {
        var criterion = new FilterCriterion({
            name: this.name, // TODO: make this private and add accessor
            value: this._value,
            test: this._test.bind({}),
            description: this._description
        });
        return criterion;
    }

    getValue(){
        return this._value;
    }

    test(boardGame) {
        if (this._value === undefined) {
            return true;
        }
        const result = this._test(boardGame, this._value);
        return result;
    }

    getDescription() {
        return this._description;
    }
}
export default FilterCriterion;
