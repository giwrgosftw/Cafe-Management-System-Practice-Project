import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders} from "@angular/common/http";

/**
 * @Injectable decorator marks this class as a service that can be injected into other components or services.
 * The 'providedIn: root' option ensures this service is available throughout the application.
 */
@Injectable({
  providedIn: 'root'
})
export class ProductService {

  url = environment.apiUrl;  // Base URL for API endpoints

  /**
   * Constructor function that initializes the ProductService.
   *
   * @param httpClient - The Angular HttpClient service used for making HTTP requests.
   */
  constructor(private httpClient: HttpClient) { }

  /**
   * Adds a new product by sending a POST request with product data to the server.
   *
   * @param data - The product data to be added.
   * @returns Observable<any> - An observable that contains the server's response.
   */
  add(data: any) {
    return this.httpClient.post(
      this.url + "/product/addNewProduct", data,
      { headers: new HttpHeaders().set('Content-Type', "application/json") }
    )
  }

  /**
   * Updates an existing product by sending a POST request with updated product data to the server.
   *
   * @param data - The updated product data.
   * @returns Observable<any> - An observable that contains the server's response.
   */
  update(data: any) {
    return this.httpClient.post(
      this.url + "/product/updateProduct", data,
      { headers: new HttpHeaders().set('Content-Type', "application/json") }
    )
  }

  /**
   * Retrieves a list of all products by sending a GET request to the server.
   *
   * @returns Observable<any> - An observable that contains the list of products.
   */
  getProducts() {
    return this.httpClient.get(this.url + "/product/getAllProducts")
  }

  /**
   * Updates the status of a product (e.g., activate or deactivate) by sending a POST request to the server.
   *
   * @param data - The data containing product ID and the new status.
   * @returns Observable<any> - An observable that contains the server's response.
   */
  updateStatus(data: any) {
    return this.httpClient.post(
      this.url + "/product/updateProduct", data,
      { headers: new HttpHeaders().set('Content-Type', "application/json") }
    )
  }

  /**
   * Deletes a product by sending a POST request with the product ID to the server.
   *
   * @param id - The ID of the product to be deleted.
   * @returns Observable<any> - An observable that contains the server's response.
   */
  delete(id: any) {
    return this.httpClient.post(
      this.url + "/product/deleteProduct?productId=" + id,
      {},  // Empty body since the productId is sent as a query parameter
      { headers: new HttpHeaders().set('Content-Type', "application/json") }
    );
  }


  /**
   * Retrieves a list of products that belong to a specific category by sending a GET request to the server.
   *
   * @param id - The ID of the category.
   * @returns Observable<any> - An observable that contains the list of products in the specified category.
   */
  getProductsByCategory(id: any) {
    return this.httpClient.get(this.url + "/product/getProductsByCategory", {
      params: { categoryId: id },
      headers: new HttpHeaders().set('Content-Type', "application/json")
    });
  }

  /**
   * Retrieves the details of a specific product by sending a GET request to the server.
   *
   * @param id - The ID of the product.
   * @returns Observable<any> - An observable that contains the product details.
   */
  getById(id: any) {
    return this.httpClient.get(this.url + "/product/getProductById", {
      params: { productId: id },
      headers: new HttpHeaders().set('Content-Type', "application/json")
    });
  }

}
