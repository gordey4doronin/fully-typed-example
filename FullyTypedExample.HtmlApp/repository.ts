import { Employee } from './models';

type Promise<T> = { then(callbackfn: (data: T) => void): any; }

export class Repository {

    /**
     * gets the list of all employees
     */
    public getEmployees(): Promise<Employee[]> {
        return (window as any).fetch('http://localhost:1234/api/employees')
            .then((response) => {
                return response.json();
            });
    }

}