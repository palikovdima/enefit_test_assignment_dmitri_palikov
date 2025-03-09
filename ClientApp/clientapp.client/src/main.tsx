import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import ProductsPageRouter from './products/ProductsPageRouter'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
        <ProductsPageRouter />
  </StrictMode>,
)
