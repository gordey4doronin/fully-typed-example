class Greeter {
    private element: HTMLElement;
    private table: HTMLElement;

    constructor(element: HTMLElement) {
        this.element = element;
        this.table = document.createElement('table');
        this.element.appendChild(this.table);
    }

    public makeRequest() {
        (<any>window).fetch('http://localhost:1234/api/employees')
            .then((response) => {
                return response.json();
            })
            .then((employees: Employee[]) => {
                // Generate html using tempalte string
                var tableHtml = employees.reduce((acc: string, x: Employee) => {
                    return `${acc}<tr><td>${x.id}</td><td>${x.name}</td></tr>`;
                }, '');

                this.table.innerHTML = tableHtml;
            });
    }
}

// ReSharper disable once InconsistentNaming
interface Employee {
    id: number;
    name: string;
}

window.onload = () => {
    var element = document.getElementById('content');
    var greeter = new Greeter(element);
    greeter.makeRequest();
};