export class FilterCriterion {
    constructor({ name, value, mapValue, test, describe }) {
        this.name = name;
        this._value = value;
        this._mapValue = mapValue ? mapValue : v => v;
        this._test = test;
        this._describe = describe;
    }

    clone() {
        var criterion = new FilterCriterion({
            name: this.name, // TODO: make this private and add accessor
            value: this._value,
            mapValue: this._mapValue.bind({}),
            test: this._test.bind({}),
            describe: this._describe.bind({})
        });
        return criterion;
    }

    test(boardGame) {
        if (this._value == undefined) {
            return true;
        }
        const result = this._test(boardGame, this._value);
        return result;
    }

    setValue(value) {
        this._value = this._mapValue(value);
    }

    getValue() {
        return this._value;
    }

    describe() {
        return this._describe(this._value);
    }

    isDefault() {
        return this._value == undefined;
    }
}
export default FilterCriterion;
