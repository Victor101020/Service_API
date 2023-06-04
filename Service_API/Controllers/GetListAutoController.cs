using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Service_API.Models; // импортдля класса Auto
using System.Reflection.Metadata.Ecma335;
using static System.Console;
using System;


namespace Service_API.Controllers
{
    [Route("api/[controller]")] //вместо controller подставится имя контроллера
    [ApiController]
    public class GetListAutoController : ControllerBase
    {
        // создаем список с автомобилями
        private static List<GetListAuto> products = new List<GetListAuto>(new[]
        {
            new GetListAuto() {Id = 1, Name = "Toyota", Price = 10000 },//создаем список из переменных класса Auto
            new GetListAuto() {Id = 2, Name = "Mercedes", Price = 20000 },
            new GetListAuto() {Id = 3, Name = "BMW", Price = 65000 },
            new GetListAuto() {Id = 4, Name = "LAda", Price = 70000 },
            new GetListAuto() {Id = 5, Name = "Opel", Price = 90000 },
            new GetListAuto() {Id = 6, Name = "Audi", Price = 40000 },
            new GetListAuto() {Id = 7, Name = "Nissan", Price = 14000 },
            new GetListAuto() {Id = 8, Name = "Kamaz", Price = 55000 },
            new GetListAuto() {Id = 9, Name = "GAZ", Price = 69000 },
            new GetListAuto() {Id = 10, Name = "YAZ", Price = 31000 }
        });
        
        //реализуем дефолтный запрос без параметров
        [HttpGet]// говорит о том, что при вызове гет этого контроллера будет без параметров будет вызван метод ниже
        public IEnumerable<GetListAuto> Get() => products;

        //реализуем запрос с параметрами
        [HttpGet("{id}")] //параметра для маршрутизации 
        public IActionResult Get(int id)
        {
            var product = products.SingleOrDefault(p => p.Id == id); // тут мф пробегаемся по списку product и смотрим чтобы предикат был равен Id - если не найдет то переменная примет значение NotFound 

            if (product == null)//проверяем если нет в списке Id то вернем NotFound
            {
                return NotFound();
            }
            return Ok(product);
        }

        //реализуем удаление элемента из списка
        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {
            products.Remove(products.SingleOrDefault(p => p.Id == id));
            return Ok(new { Message = "Deleted successfully"});
        }

        //создадим класс где будем искать свободный идентификатор 
        //создадим переменную-число последнего эл-а в списке
        private int NextProductId =>
            products.Count() ==0 ? 1 :products.Max(x => x.Id) +1; //если список пустой  - присваиваем 1, иначе ищем максимальный  индекс и прибаляем 1

        //реализация гет запроса для поиска своблдного индекса
        [HttpGet("GetNextProductId")]
        public int GetNextProductId() 
        {
            {
                return NextProductId; 
            }
        }

        //реализуем метод POST в который параметры передаются в виде полей формы: curl -X POST -F Name=someName -F Price=123 http://localhost:44386/api/products/
        [HttpPost]
        public IActionResult Post(GetListAuto product) //product берется из полей формы GetListAuto - должны бать обяз параметры формы
        {
            if (!ModelState.IsValid) //проверка на валидность переданных параметров согласно модели GetListAuto
            {
                return BadRequest(ModelState);
            }
            product.Id = NextProductId; //берем доступный идентификатор
            products.Add(product);  //добавляем товар в нашу коллекцию
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);//это нужно чтобы выполнился гет и вернуть в ответ метода новый товар
        }

        //Реализуем метод пост в который параметры передаются ввиде JSON curl -X POST -H "Content-Type:application/json" -d "{\"name":\"JsonName",\"price":1234}" http://localhost:44386/api/products/
        [HttpPost("AddProduct")]
        public IActionResult PostBody([FromBody] GetListAuto product) => //получить параметры из тела запроса [FromBody] согласно модели GetListAuto и вернуть в переменную product
            Post(product);//тут вызываем метод пост реализованный на 61 строке, чтобы добавить значения


        //реализуем метод изменения товаров по формам (аналогично посту можно реализовать и передачу Json)
        [HttpPut]
        public IActionResult Put (GetListAuto product)
        {
            if (!ModelState.IsValid) //првоеряем что полученный объект валидный согласно модели GetListAuto
            {
                return BadRequest(ModelState); 
            }
            var storeProduct = products.SingleOrDefault(p => p.Id == product.Id); //ищем Id объекта и присваиваем в переменную storeProduct
            if (storeProduct == null) 
                return NotFound(); //если не находим объект по id то, возвращаем NotFound
            //а если нашли, то:
            storeProduct.Name = product.Name;//меняем имя
            storeProduct.Price = product.Price;//меняем цену
            return Ok(storeProduct);//возвращаем в ответ измененный товар
        }

        //реализуем метод Put с передачей параметров как Json
        [HttpPut("UpdateProduct")]
        public IActionResult PutBody([FromBody] GetListAuto product) => //получить параметры из тела запроса [FromBody] согласно модели GetListAuto и вернуть в переменную product
            Put(product);//тут вызываем метод пост реализованный на 61 строке, чтобы добавить значения

    }
}
